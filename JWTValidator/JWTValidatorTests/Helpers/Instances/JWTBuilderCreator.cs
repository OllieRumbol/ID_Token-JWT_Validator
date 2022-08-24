using JWTValidatorTests.Helpers.Interfaces;
using System;

namespace JWTValidatorTests.Helpers.Instances;

public class JWTBuilderCreator
{
    private IJWTOptionsBuilder jWTBuilder;

    private JWTBuilderCreator()
    {
        jWTBuilder = new JWTOptionsBuilder();
    }

    public static JWTBuilderCreator Create()
    {
        return new JWTBuilderCreator();
    }

    public JWTBuilderFinisher WithSecret(String Secret)
    {
        jWTBuilder.WithSecret(Secret);
        return new JWTBuilderFinisher(jWTBuilder);
    }
}