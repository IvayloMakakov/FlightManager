using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using FlightManager.Models;
using System.Reflection.Emit;

namespace FlightManager.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Flight> Flights { get; set; }
		public DbSet<Passanger> Passengers { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<PassangerReservation> PassengersReservations { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Passanger>()
			   .HasIndex(b => b.EGN)
			.IsUnique();

			builder.Entity<PassangerReservation>()
			.HasKey(x => new { x.PassangerId, x.ReservationId });

			builder.Entity<PassangerReservation>()
				.HasOne(pr => pr.Passanger)
				.WithMany(p => p.Reservations)
				.HasForeignKey(pr => pr.PassangerId);

			builder.Entity<PassangerReservation>()
				.HasOne(pr => pr.Reservation)
				.WithMany(r => r.Passengers)
				.HasForeignKey(pr => pr.ReservationId);

			base.OnModelCreating(builder);
		}
		public DbSet<FlightManager.Models.User> User { get; set; }
	}
}
