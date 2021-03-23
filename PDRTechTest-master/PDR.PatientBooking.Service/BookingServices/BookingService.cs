using Microsoft.EntityFrameworkCore;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using static PDR.PatientBooking.Service.BookingServices.Responses.GetAllBookingsResponse;

namespace PDR.PatientBooking.Service.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly PatientBookingContext _context;
        private readonly IAddBookingRequestValidator _validator;

        public BookingService(PatientBookingContext context, IAddBookingRequestValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public void AddBooking(AddBookingRequest request)
        {
            var validationResult = _validator.ValidateRequest(request);

            if (!validationResult.PassedValidation)
            {
                throw new ArgumentException(validationResult.Errors.First());
            }
            var bookingId = new Guid();
            var bookingStartTime = request.StartTime;
            var bookingEndTime = request.EndTime;
            var bookingPatientId = request.PatientId;
            var bookingPatient = _context.Patient.FirstOrDefault(x => x.Id == request.PatientId);
            var bookingDoctorId = request.DoctorId;
            var bookingDoctor = _context.Doctor.FirstOrDefault(x => x.Id == request.DoctorId);
            //var bookingSurgeryType = _context.Patient.FirstOrDefault(x => x.Id == bookingPatientId).Clinic.SurgeryType;

            var myBooking = new Order
            {
                Id = bookingId,
                StartTime = bookingStartTime,
                EndTime = bookingEndTime,
                PatientId = bookingPatientId,
                DoctorId = bookingDoctorId,
                Patient = bookingPatient,
                Doctor = bookingDoctor,
                //SurgeryType = (int)bookingSurgeryType
            };

            _context.Order.AddRange(new List<Order> { myBooking });
            _context.SaveChanges();
            _context.SaveChanges();
        }

        public GetAllBookingsResponse GetAllBookings(long identificationNumber)
        {
            var bockings = _context.Order.OrderBy(x => x.StartTime).ToList();

            if (bockings.Where(x => x.Patient.Id == identificationNumber).Count() == 0)
            {
                return null;
            }
            else
            {
                var bookings2 = bockings.Where(x => x.PatientId == identificationNumber);
                if (bookings2.Where(x => x.StartTime > DateTime.Now).Count() == 0)
                {
                    return null;
                }
                else
                {
                    var bookings3 = bookings2.Where(x => x.StartTime > DateTime.Now);
                    GetAllBookingsResponse ret = new GetAllBookingsResponse();
                    ret.Bookings = new List<MyOrderResult>();
                    ret.Bookings.Add(new MyOrderResult
                    {
                        Id = bookings3.First().Id,
                        DoctorId = bookings3.First().DoctorId,
                        StartTime = bookings3.First().StartTime,
                        EndTime = bookings3.First().EndTime
                    });
                    return ret;
                }
            }

         
        }

        public void CancelBooking(long identificationNumber,DateTime startTime)
        {
            
            if (startTime > DateTime.Now)
            {
                if (_context.Order.Where(x => x.Patient.Id == identificationNumber && x.StartTime == startTime).Count() == 0)
                {
                    var appointment = _context.Order.Where(x => x.Patient.Id == identificationNumber && x.StartTime == startTime).SingleOrDefault();
                    _context.Order.Remove(appointment);
                }

            }
       
        }
    }
}
