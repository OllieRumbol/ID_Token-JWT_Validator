using JWTValidatorTests.Helpers.Interfaces;
using System;

namespace JWTValidatorTests.Helpers.Instances;

public class JWTOptionsBuilderCreator
{
    private IJWTOptionsBuilder jWTBuilder;

    private JWTOptionsBuilderCreator()
    {
        jWTBuilder = new JWTOptionsBuilder();
    }

    public static JWTOptionsBuilderCreator Create()
    {
        return new JWTOptionsBuilderCreator();
    }

    public JWTOptionsBuilderFinisher WithSecret(String Secret)
    {
        jWTBuilder.WithSecret(Secret);
        return new JWTOptionsBuilderFinisher(jWTBuilder);
    }
}