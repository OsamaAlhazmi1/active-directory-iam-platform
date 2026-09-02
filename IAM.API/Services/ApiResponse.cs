using System;

namespace AD_web_project.Services;

public static class ApiResponse
{
    public static IResult Success(string message , object? data =null)
   => Results.Ok(new
        {
            success = true,
            message,
            data
        });

    public static IResult Fail(string message , int statusCode =400)
   => Results.Json(new
        {
            success = false,
            message,
            
        },statusCode:statusCode);
}
