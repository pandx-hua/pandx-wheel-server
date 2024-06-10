using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Validation;

namespace pandx.Wheel.Exceptions;

public class MyProblemDetails : ProblemDetails
{
    public List<ValidationError> ValidationErrors { get; set; } = default!;
}