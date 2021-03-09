using PDR.PatientBooking.Data;
using PDR.PatientBooking.Service.DoctorServices.Requests;
using PDR.PatientBooking.Service.Enums;
using PDR.PatientBooking.Service.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace PDR.PatientBooking.Service.DoctorServices.Validation
{
    public class AddDoctorRequestValidator : IAddDoctorRequestValidator
    {
        private readonly PatientBookingContext _context;

        public AddDoctorRequestValidator(PatientBookingContext context)
        {
            _context = context;
        }

        public PdrValidationResult ValidateRequest(AddDoctorRequest request)
        {
            var result = new PdrValidationResult(true);

            if (MissingRequiredFields(request, ref result))
                return result;

            if (DoctorEmailValidation(request, ref result))
                return result;

            if (DoctorAlreadyInDb(request, ref result))
                return result;

            if (DoctorGenderValidation(request, ref result))
                return result;

            return result;
        }

        public bool MissingRequiredFields(AddDoctorRequest request, ref PdrValidationResult result)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(request.FirstName))
                errors.Add("FirstName must be populated");

            if (string.IsNullOrEmpty(request.LastName))
                errors.Add("LastName must be populated");

            if (string.IsNullOrEmpty(request.Email))
                errors.Add("Email must be populated");

           

            if (errors.Any())
            {
                result.PassedValidation = false;
                result.Errors.AddRange(errors);
                return true;
            }

            return false;
        }

        private bool DoctorAlreadyInDb(AddDoctorRequest request, ref PdrValidationResult result)
        {
            if (_context.Doctor.Any(x => x.Email == request.Email))
            {
                result.PassedValidation = false;
                result.Errors.Add("A doctor with that email address already exists");
                return true;
            }

            return false;
        }

        private bool DoctorGenderValidation(AddDoctorRequest request, ref PdrValidationResult result)
        {
            if (Enum.IsDefined(typeof(Gender), request.Gender))
            {
                return false;
            }
            else
            {
                result.PassedValidation = false;
                result.Errors.Add("Gender value not valid  0:Female 1:Male 2:Other");
                return true;
            }
        }

        private bool DoctorEmailValidation(AddDoctorRequest request, ref PdrValidationResult result)
        {

            var email = request.Email;
        
         
                if (email == "")
                {
                    result.PassedValidation = false;
                    result.Errors.Add("Email must be a valid email address");
                    return true;
                }
                else if (email == null)
                {
                    result.PassedValidation = false;
                    result.Errors.Add("Email must be populated");
                    return true;
                }

            

            if (email=="")
            {
                result.PassedValidation = false;
                result.Errors.Add("Email must be a valid email address");
                return true;
            }
            try
            {
                MailAddress m = new MailAddress(email);

                return false;
            }
            catch (FormatException)
            {
                result.PassedValidation = false;
                result.Errors.Add("Email must be a valid email address");
                return true;
            }
        }


    }
}
