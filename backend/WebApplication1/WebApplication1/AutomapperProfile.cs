using AutoMapper;
using Database.Models;
using WebApplication1.Controllers.DTOs;

namespace WebApplication1.Mapping;

public static class AutoMappingProfile
{
   public static void UseAutoMappingProfile(IMapperConfigurationExpression cfg){
        // Customer mappings
        cfg.CreateMap<Customer, CustomerDto>();
        cfg.CreateMap<Customer, CustomerWithAppointmentsDto>();
        cfg.CreateMap<CreateCustomerDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Barber mappings
        cfg.CreateMap<Barber, BarberDto>();
        cfg.CreateMap<CreateBarberDto, Barber>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // BarberSchedule mappings
        cfg.CreateMap<BarberSchedule, BarberScheduleDto>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")));

        cfg.CreateMap<BarberScheduleDto, BarberSchedule>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.Parse(src.EndTime)));

        // Appointment mappings
        cfg.CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.StartTime.AddMinutes(src.DurationMinutes)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        cfg.CreateMap<Appointment, AppointmentSummaryDto>()
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.StartTime.AddMinutes(src.DurationMinutes)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.Name));

        cfg.CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AppointmentStatus.Booked))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.ServiceType.ToLower() == "both" ? 60 : 30));
    }
}