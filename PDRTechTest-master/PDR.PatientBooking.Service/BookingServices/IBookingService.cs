using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Responses;
using System;

namespace PDR.PatientBooking.Service.BookingServices
{
    public interface IBookingService
    {
        void AddBooking(AddBookingRequest request);
        GetAllBookingsResponse GetAllBookings(long identificationNumber);
        void CancelBooking(long identificationNumber, DateTime startTime);
    }
}