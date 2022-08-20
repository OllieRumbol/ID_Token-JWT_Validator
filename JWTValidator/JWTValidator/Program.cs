﻿using JWTValidatorModel;
using JWTValidatorService;
using JWTValidatorService.Extensions;

String JWT = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkMxQ0M1RTYxNzNDMjRGMzEyQzMyRjlGRjY5QkI5Mzg5RDAxOThFNjUiLCJ0eXAiOiJhdCtqd3QiLCJ4NXQiOiJ3Y3hlWVhQQ1R6RXNNdm5fYWJ1VGlkQVpqbVUifQ.eyJuYmYiOjE2NjA1MDYxNzAsImV4cCI6MTY2MDUwOTc3MCwiaXNzIjoiaHR0cHM6Ly9sb2dpbi5wZXJzb25pZnlnby5jb20vcHJvZGFycmwiLCJjbGllbnRfaWQiOiI2ZTRmYTk0ZS0xMGE1LTQ2YTQtYjkwZi1kMTMxMGNhY2RlYTMiLCJjbGllbnRfcm9sZSI6IjE2MSIsImNsaWVudF9hbGxzY29wZXMiOiJUcnVlIiwiY2xpZW50X3VwbiI6IndlYmFkbWluIiwiY2xpZW50X29yZyI6IkFSUkwiLCJjbGllbnRfb3JndW5pdCI6IkFSUkwiLCJjbGllbnRfYWN0b3IiOiIzMjAwMzgzMDM5fDAiLCJzdWIiOiIyMDE2ZDMwMy1jNTljLTRlOTgtYmJhMS00MjU0ZDhkMmRjZmMiLCJhdXRoX3RpbWUiOjE2NjA1MDYxNTgsImlkcCI6ImxvY2FsIiwibmFtZSI6IlRlc3Qud2ViLnN1YnMiLCJBc3BOZXQuSWRlbnRpdHkuU2VjdXJpdHlTdGFtcCI6IjJCSVRQM1RHRjdGUUs2UUFLT0w2Uk5TVlNBQzVNTU9KIiwiYWN0b3IiOiIzMjAwMzgzMDM5fDAiLCJlbWFpbCI6InNhbXRlc3QuYXJybCt0ZXN0QGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjoiVHJ1ZSIsInBob25lX251bWJlcl92ZXJpZmllZCI6IkZhbHNlIiwicm9sZSI6Ik1FTUJFUiIsIk1hc3RlckN1c3RvbWVySWQiOiIzMjAwMzgzMDM5IiwiU3ViQ3VzdG9tZXJJZCI6IjAiLCJSZWNvcmRUeXBlIjoiSSIsIlByaW1hcnlFbWFpbEFkZHJlc3MiOiJzYW10ZXN0LmFycmwrdGVzdEBnbWFpbC5jb20iLCJnaXZlbl9uYW1lIjoiVGVzdCIsImZhbWlseV9uYW1lIjoiQWNjZXNzIiwiVW5pcXVlS2V5IjoiMzIwMDM4MzAzOXwwIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsInVzZXJkYXRhIiwiZW1haWwiLCJyb2xlIiwicGhvbmVudW1iZXIiXSwiYW1yIjpbInB3ZCJdfQ.Pc1RCoCwB8JksHgCZY9XmmRLxwDGQ_6Xt83CcPD_ep7lh5VvGgs40Os04JN916Iygx2Qm9Xs_goP5lFb0d2s7gx0NEE86Eu9BPS7l_5aVJjSJCfBPN75cc2ldXAoCmVB1mNxWebxSTlprl6LAnbACSX9l_YWokcN96-ppVmTc_w4ZAAIXS6uOBx7zXWa9-7Af3qMFSTZ6PfQRBFs0fOsPPqsD496k5l6wwjOpb0DuOdJHRbxtSr2E0rnZU-H3p8WSou8y5HiSARqh2eRcaLokx2UssugvAtchAQycmoxQkw2LGBZKz1zuaaD2mx4xKz3VliITPKdRvfl5kZK4uFrkg";

JWTValidatorOptions jWTValidatorOptions = JWTValidatorOptionsBuilder
    .Create()
    .WithSigningKeyFromOpenIdUrl("https://login.personifygo.com/prodarrl/.well-known/openid-configuration")
    //.WithExpiryDate()
    .Build();

Console.WriteLine(jWTValidatorOptions.Print());

if(new JWTValidator().TryValidateJWT(JWT, jWTValidatorOptions, out Dictionary<String, List<String>> result) == false)
{
    Console.WriteLine("Invalid JWT");
    return;
}

Console.WriteLine(result.Print());

Boolean hasMemberRole = result.DictionaryContainsKeyAndValue("role", "MEMBER");

Console.WriteLine(hasMemberRole);