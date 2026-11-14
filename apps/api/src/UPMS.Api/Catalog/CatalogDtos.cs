using UPMS.Api.Data;

namespace UPMS.Api.Catalog;

public record RouteDto(Guid Id, string Code, string Name, bool Active, int StopCount);
public record RouteStopDto(Guid Id, Guid StopId, string StopName, int Sequence, int OffsetMinutes);
public record RouteDetailDto(Guid Id, string Code, string Name, bool Active, IReadOnlyList<RouteStopDto> Stops);
public record RouteRequest(string Code, string Name, bool Active = true);

public record StopDto(Guid Id, string Name, double Latitude, double Longitude);
public record StopRequest(string Name, double Latitude, double Longitude);

public record VendorDto(Guid Id, string Name, VendorStatus Status, int BusCount);
public record VendorRequest(string Name, VendorStatus Status);

public record BusDto(Guid Id, string Registration, int Capacity, Ownership Ownership, Guid? VendorId, string? VendorName);
public record BusRequest(string Registration, int Capacity, Ownership Ownership, Guid? VendorId);

public record TripDto(Guid Id, Guid RouteId, string RouteCode, DateTime DepartureAt, decimal Fare, TripStatus Status);
public record TripRequest(Guid RouteId, DateTime DepartureAt, decimal Fare, TripStatus Status = TripStatus.Scheduled);

public record RouteStopRequest(Guid StopId, int Sequence, int OffsetMinutes);
