using System;

namespace JWTValidatorTests.Helpers.Builder;

public class JWTOptionsBuilderCreator : IJWTOptionsBuilderCreator
{
    private IJWTOptionsBuilder JwtOptionBuilder;

    private JWTOptionsBuilderCreator() => JwtOptionBuilder = new JWTOptions.JWTOptionsBuilder();

    public static JWTOptionsBuilderCreator Create() => new JWTOptionsBuilderCreator();

    public IJWTOptionsBuilderFinisher WithSecret(String Secret)
    {
        JwtOptionBuilder.WithSecret(Secret);
        return new JWTOptionsBuilderFinisher(JwtOptionBuilder);
    }
}