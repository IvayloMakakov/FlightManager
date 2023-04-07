using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightManager.Data;
using FlightManager.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using FlightManager.Addings;
using FlightManager.ViewModels.PassangerReservationViewModel;
using System.Text.Encodings.Web;
using System.Text;

namespace FlightManager.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IEmailSender emailSender;
        public ReservationsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            this.emailSender = emailSender;
		}

		// GET: Reservations
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Index(string emailSearch, string currentFilter, int? pageNumber)
		{
			if (emailSearch != null)
			{
				pageNumber = 1;
			}
			else
			{
				emailSearch = currentFilter;
			}
			ViewData["EmailSearch"] = emailSearch;

			var reservations = from r in _context.Reservations
							   select r;
			if (!String.IsNullOrEmpty(emailSearch))
			{
				reservations = reservations.Where(r => r.Email.Contains(emailSearch));
			}

			int pageSize = 3;
			return View(await PaginatedList<Reservation>.CreateAsync(reservations.Include(r => r.Flight), pageNumber ?? 1, pageSize));
		}

		// GET: Reservations/Details/5
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Flight)
                .Include(p=>p.Passengers)
                .ThenInclude(p=>p.Passanger)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,Email,IsConfirmed,FlightId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
				var passengerCount = int.Parse(this.Request.Form.FirstOrDefault(x => x.Key == "PassengerCount").Value);
				return RedirectToAction(nameof(AddPassengers), new { count = passengerCount, reservation = reservation.ReservationId });
			}
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", reservation.FlightId);
            return View(reservation);
        }
        // GET: Query
		private async Task<IActionResult> AddPassengers()
		{
			int count = int.Parse(this.Request.Query["count"]);
			int reservationId = int.Parse(this.Request.Query["reservation"]);

			var reservation = await _context.Reservations.FindAsync(reservationId);
			var flight = await _context.Flights.FindAsync(reservation.FlightId);
			ViewData.Add("BusinessSeats", flight.BusinessCapacity);
			ViewData.Add("RegularSeats", flight.PassengerCapacity);

			var passengerReservationViewModel = new PassangerReservationViewModel() { PassengerCapacity = count, CurrentCount = count, ReservationId = reservationId };
			return View(passengerReservationViewModel);
		}
		// POST: Reservations/AddPassengers
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPassengers(int? rnd = null)
		{
			var model = new PassangerReservationViewModel(this.Request.Form);
			var reservation = await _context.Reservations
				.Include(r => r.Flight)
				.Include(r => r.Passengers)
				.FirstOrDefaultAsync(m => m.ReservationId == model.ReservationId);
			var flight = await _context.Flights.FindAsync(reservation.FlightId);
			ViewData.Add("BusinessSeats", flight.BusinessCapacity);
			ViewData.Add("RegularSeats", flight.PassengerCapacity);

			if (ModelState.IsValid)
			{
				var passenger = await _context.Passengers.FirstOrDefaultAsync(x => x.EGN == model.EGN);
				if (passenger == null)
				{
					var newPassenger = new Passanger()
					{
						FirstName = model.FirstName,
						LastName = model.LastName,
						MiddleName = model.MiddleName,
						Nationality = model.Nationality,
						EGN = model.EGN,
						PhoneNumber = model.PhoneNumber
					};
					_context.Passengers.Add(newPassenger);
					await _context.SaveChangesAsync();
					passenger = await _context.Passengers.FindAsync(newPassenger.PassangerId);
				}

				int capacity = 0;
				TicketsType? ticketType = null;
				if (model.TicketType == 1) {
					capacity = reservation.Flight.PassengerCapacity - reservation.Passengers.Where(r => r.TicketsType == TicketsType.Normal).Count();
					ticketType = TicketsType.Normal;
				}
				else
				{
					capacity = reservation.Flight.PassengerCapacity - reservation.Passengers.Where(r => r.TicketsType == TicketsType.Business).Count();
					ticketType = TicketsType.Business;

				}
				if (capacity > 0)
				{
					var relation = new PassangerReservation()
					{
						Passanger = passenger,
						PassangerId = passenger.PassangerId,
						ReservationId = reservation.ReservationId,
						Reservation = reservation,
						TicketsType = (TicketsType)ticketType
					};

					reservation.Passengers.Add(relation);
					passenger.Reservations.Add(relation);
					await _context.SaveChangesAsync();
				}

				model = new PassangerReservationViewModel()
				{
					ReservationId = model.ReservationId,
					CurrentCount = model.CurrentCount - 1,
					PassengerCapacity = model.PassengerCapacity
				};
			}

			if (model.CurrentCount == 0)
			{
				var callbackUrl = Url.Action(
						"Confirm",
						"Reservations",
						values: new { id = reservation.ReservationId },
						protocol: Request.Scheme);

				reservation = await _context.Reservations
					.Include(r => r.Flight)
					.Include(r => r.Passengers)
					.ThenInclude(p => p.Passanger)
					.FirstOrDefaultAsync(m => m.ReservationId == model.ReservationId);

				var message = new StringBuilder();
				message.Append($"Confirm your reservation <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Here</a>. <br /> {reservation.Passengers.Count} passangers count by flight {reservation.Flight.FlightId}. List: <br />");
				foreach (var passengerReservation in reservation.Passengers)
				{
					var passenger = passengerReservation.Passanger;
					message.Append($"{passenger.EGN} {passenger.FirstName} {passenger.MiddleName} {passenger.LastName} {passengerReservation.TicketsType}");
					message.Append("<br />");
				}
				await this.emailSender.SendEmailAsync(reservation.Email, "Confirm reservation", message.ToString());
				return View("ConfirmationNeeded");
			}

			return View(model);
		}
		public async Task<IActionResult> Confirm(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var reservation = await _context.Reservations
				.Include(r => r.Flight)
				.Include(r => r.Passengers)
				.FirstOrDefaultAsync(m => m.ReservationId == id);
			if (reservation == null)
			{
				return NotFound();
			}

			reservation.IsConfirmed = true;
			reservation.Flight.PassengerCapacity -= reservation.Passengers.Where(r => r.TicketsType == TicketsType.Normal).Count();
			reservation.Flight.BusinessCapacity -= reservation.Passengers.Where(r => r.TicketsType == TicketsType.Business).Count();
			try
			{
				_context.Update(reservation);
				var result = await _context.SaveChangesAsync();
				return View();
			}
			catch 
			{
				throw;
			}
		}
		// GET: Reservations/Edit/5
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", reservation.FlightId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Edit(int id, [Bind("ReservationId,Email,IsConfirmed,FlightId")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FlightId"] = new SelectList(_context.Flights, "FlightId", "FlightId", reservation.FlightId);
            return View(reservation);
        }

		// GET: Reservations/Delete/5
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Flight)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
