﻿using JWTValidatorModel;

namespace JWTValidatorService.Interface;
public interface IJWTValidator
{
    Boolean TryValidateJWT(String jwt, JWTValidatorOptions options, out Dictionary<String, List<String>> result);

    Dictionary<String, List<String>> ValidateJWT(String jwt, JWTValidatorOptions options);
}