using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.ClinicServices.Requests;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDR.PatientBooking.Service.BookingServices.Validation
{
    public class AddBookingRequestValidator : IAddBookingRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddBookingRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddBookingRequest request)
        {
            var result = new PdrValidationResult(true);

            if (DateValidation(request, ref result))
                return result;

            if (BookingAlreadyInDb(request, ref result))
                return result;

            return result;
        }

        public bool DateValidation(AddBookingRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (request.StartTime==null)
            {
                result.PassedValidation = false;
                errors.Add("StartTime must be populated");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }
               
            if (request.EndTime == null)
            {
                errors.Add("End Time must be populated");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }
               

            if (request.StartTime > DateTime.Today)
            {
                errors.Add("Start Time must be in the Future");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }
            if (request.EndTime > DateTime.Today)
            {
                errors.Add("End Time must be in the Future");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }
            if (request.StartTime > DateTime.Today)
            {
                errors.Add("Start Time must be in the Future");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }
            if (request.StartTime >= request.EndTime)
            {
                errors.Add("start Time must before end Time");
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool BookingAlreadyInDb(AddBookingRequest request, ref PdrValidationResult result)
        {
            if (_context.Order.Any(x => x.StartTime == request.StartTime && x.DoctorId == request.DoctorId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A Booking with that Start Time already exists");
                return true;
            }
            if (_context.Order.Any(x => x.EndTime == request.EndTime && x.DoctorId == request.DoctorId))
            {
                result.PassedValidation = false;
                result.Errors.Add("A Booking with that End Time already exists");
                return true;
            }

            return false;
        }
    }
}
