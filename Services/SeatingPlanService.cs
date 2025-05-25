using System;
using System.Collections.Generic;
using System.Linq;
using UniversityAffairs.Data;
using UniversityAffairs.Models;

namespace UniversityAffairs.Services
{
    public class SeatingPlanService
    {
        private readonly UniversityDbContext _context;

        public SeatingPlanService(UniversityDbContext context)
        {
            _context = context;
        }

        // Amfi düzeni: 10 sıra, her sırada 11 koltuk (4-3-4)
        private const int TotalRows = 10;
        private const int TotalColumns = 11;

        private readonly Dictionary<string, List<int>> AllowedColumnsPerBlock = new()
        {
            { "Left", new List<int> { 0, 2 } },
            { "Middle", new List<int> { 4, 6 } },
            { "Right", new List<int> { 7, 9 } }
        };


        public List<SeatingPlan> GenerateSeatingPlan(int examScheduleId, List<string> studentNumbers)
        {
            var random = new Random();
            var seatingPlan = new List<SeatingPlan>();

            // Kullanılabilir koltuk pozisyonları (blok kuralına göre)
            var allowedSeats = new List<(int Row, int Col)>();

            for (int row = 0; row < TotalRows; row++)
            {
                // Sol blok
                allowedSeats.Add((row, 0));
                allowedSeats.Add((row, 3));

                // Orta blok
                allowedSeats.Add((row, 4));
                allowedSeats.Add((row, 6));

                // Sağ blok
                allowedSeats.Add((row, 7));
                allowedSeats.Add((row, 10));
            }

            // Karıştır
            var shuffledSeats = allowedSeats.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < Math.Min(studentNumbers.Count, shuffledSeats.Count); i++)
            {
                var (row, col) = shuffledSeats[i];

                seatingPlan.Add(new SeatingPlan
                {
                    ExamScheduleId = examScheduleId,
                    StudentNumber = studentNumbers[i],
                    Row = row,
                    Column = col
                });
            }

            _context.SeatingPlans.AddRange(seatingPlan);
            _context.SaveChanges();

            return seatingPlan;
        }

    }
}
