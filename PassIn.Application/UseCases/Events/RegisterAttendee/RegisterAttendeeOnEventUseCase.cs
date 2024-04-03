﻿using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;
    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);

        var entity = new Infrastructure.Entities.Attendee
        {
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisteredJson
        {
            Id = entity.Event_Id,
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);

        if(eventEntity is null)
            throw new NotFoundException("An event with this id does not exist.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ErrorOnValidationException("The Name is invalid");

        if (EmailIsValid(request.Email) == false )
            throw new ErrorOnValidationException("The e-mail is invalid");

        var attendeeAlreadyRegister = _dbContext.Attendees.Any(attendee => attendee.Email.Equals(request.Email) && attendee.Id == eventId);
        if (attendeeAlreadyRegister)
            throw new ConflitException("You can not register twice on the same event.");

        var attendeesForEvent = _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);
        if (attendeesForEvent > eventEntity.Maximum_Attendees)
            throw new ConflitException("There ir no room for this event.");
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
    }
}