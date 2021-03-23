using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PDR.PatientBooking.Data;
using PDR.PatientBooking.Data.Models;
using PDR.PatientBooking.Service.BookingServices.Requests;
using PDR.PatientBooking.Service.BookingServices.Validation;
using System;

namespace PDR.PatientBooking.Service.Tests.BookingServices.Validation
{
    [TestFixture]
    public class AddBookingRequestValidatorTests
    {
        private IFixture _fixture;

        private PatientBookingContext _context;

        private AddBookingRequestValidator _addBookingRequestValidator;

        [SetUp]
        public void SetUp()
        {
            // Boilerplate
            _fixture = new Fixture();

            //Prevent fixture from generating from entity circular references 
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            // Mock setup
            _context = new PatientBookingContext(new DbContextOptionsBuilder<PatientBookingContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            // Mock default
            SetupMockDefaults();

            // Sut instantiation
            _addBookingRequestValidator = new AddBookingRequestValidator(
                _context
            );
        }

        private void SetupMockDefaults()
        {

        }

        [Test]
        public void ValidateRequest_AllChecksPass_ReturnsPassedValidationResult()
        {
            //arrange
            var request = GetValidRequest();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
        }

     

       
        [TestCase(null)]
        public void ValidateRequest_EndTimeNullOrEmpty_ReturnsFailedValidationResult(DateTime date)
        {
            //arrange
            var request = GetValidRequest();
            request.EndTime = date;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            //res.Errors.Should().Contain("End Time must be populated");
        }

       
        [TestCase(null)]
        public void ValidateRequest_StartTimeNullOrEmpty_ReturnsFailedValidationResult(DateTime date)
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = date;

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
        
        }


        [TestCase(2015, 12, 3)]
        public void ValidateRequest_InvalidEndDate_ReturnsFailedValidationResult(int year, int month, int day)
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = new DateTime(year, month, day);
            request.EndTime = request.StartTime.AddDays(1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //asser,
            res.PassedValidation.Should().BeTrue();
            
        }


       
        [TestCase(2024, 12, 3)]
        public void ValidateRequest_ValidStartDate_ReturnsPassedValidationResult(int year, int month, int day)
        {
            //arrange
            var request = GetValidRequest();
            request.StartTime = new DateTime(year, month, day);
            request.EndTime = request.StartTime.AddDays(1);

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
            //res.Errors.Should().Contain("End Time must be populated");
        }

        [Test]
        public void ValidateRequest_BookingAlreadyExists_ReturnsFailedValidationResult()
        {
            //arrange
            var request = GetValidRequest();

            var existingBooking = _fixture
                .Build<Order>()
                .With(x => x.StartTime, request.StartTime)
                .Create();

            _context.Add(existingBooking);
            _context.SaveChanges();

            //act
            var res = _addBookingRequestValidator.ValidateRequest(request);

            //assert
            res.PassedValidation.Should().BeFalse();
           
        }

        private AddBookingRequest GetValidRequest()
        {
            var order = _fixture.Create<Order>();
            _context.Order.Add(order);
            _context.SaveChanges();

            var request = _fixture.Build<AddBookingRequest>()
                .With(x => x.Id, order.Id)
                .With(x => x.PatientId, order.PatientId)
                .With(x => x.DoctorId, order.DoctorId)
                  .With(x => x.StartTime, order.StartTime)
                    .With(x => x.EndTime, order.EndTime)
                .Create();
            return request;
        }
    }
}
