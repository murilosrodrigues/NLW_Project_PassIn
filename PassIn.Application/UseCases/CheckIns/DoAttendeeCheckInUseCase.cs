using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.CheckIns;

public class DoAttendeeCheckInUseCase
{

    private readonly PassInDbContext _dbContext;

    public DoAttendeeCheckInUseCase()
    {
        _dbContext = new PassInDbContext();
    }


    public ResponseRegisteredJson Execute(Guid attenddeId)
    {
        Validate(attenddeId);

        var entity = new CheckIn
        { 
            Attendee_Id = attenddeId,
            Created_at = DateTime.UtcNow,
        };

        _dbContext.Checkins.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson { 
            Id = entity.Id,
        };
    }

    private void Validate(Guid attendeeId)
    {

        var existAttendee1 = _dbContext.Attendees.ToList();
        var existAttendee = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);
        if (existAttendee == false)
            throw new NotFoundException("The attendee with this Id was not found.");

        var existCheckIn = _dbContext.Checkins.Any(ch => ch.Attendee_Id == attendeeId);
        if (existCheckIn)
            throw new ConflictException("Attendee can not do checking twice in the same event.");


    }
}
