using Moq;
using Xunit;
using TicketBookingCore;

namespace TicketBookingCore.Tests
{
    public class TicketBookingRequestProcessorTests
    {
        private readonly Mock<ITicketBookingRepository> _ticketBookingRepositoryMock;
        private readonly TicketBookingRequestProcessor _processor;
        private readonly TicketBookingRequest _request;

        public TicketBookingRequestProcessorTests()
        {
            _ticketBookingRepositoryMock = new Mock<ITicketBookingRepository>();
            _processor = new TicketBookingRequestProcessor(_ticketBookingRepositoryMock.Object);
            _request = new TicketBookingRequest
            {
                FirstName = "Nevena",
                LastName = "Kicanovic",
                Email = "nevena@contactus.se"
            };
        }

        [Fact]
        public void ShouldReturnTicketBookingResultWithRequestValues()
        {
            //Arrange 

            //Act
            TicketBookingResponse response = _processor.Book(_request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(_request.FirstName, response.FirstName);
            Assert.Equal(_request.LastName, response.LastName);
            Assert.Equal(_request.Email, response.Email);
        }
        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            //Arrange
            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.Book(null));
            //Assert
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveToDatabase()
        {
            //Arrange
            TicketBooking savedTicketBooking = null;

            //Setup the Save method to capture the saved ticket booking
            _ticketBookingRepositoryMock.Setup(x => x.Save(It.IsAny<TicketBooking>())).Callback<TicketBooking>((ticketBooking) =>
            {
                savedTicketBooking = ticketBooking;
            });
            //Act
            TicketBookingResponse response = _processor.Book(_request);

            //Assert

            //Verify that the Save method was called once
            _ticketBookingRepositoryMock.Verify(x => x.Save(It.IsAny<TicketBooking>()), Times.Once);

            Assert.NotNull(savedTicketBooking);
            Assert.Equal(_request.FirstName, savedTicketBooking.FirstName);
            Assert.Equal(_request.LastName, savedTicketBooking.LastName);
            Assert.Equal(_request.Email, savedTicketBooking.Email);
        }

        [Fact]
        public void ShouldReturnErrorForInvalidEmail()
        {
            var request = new TicketBookingRequest
            {
                FirstName = "Larsa",
                LastName = "Tahir",
                Email = "LarsaTahir"
            };

            var response = _processor.Book(request);

            Assert.False(response.Works);

            _ticketBookingRepositoryMock.Verify(x => x.Save(It.IsAny<TicketBooking>()),Times.Never);

        }                              
    }
}
