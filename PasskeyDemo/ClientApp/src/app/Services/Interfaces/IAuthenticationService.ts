import {Observable} from "rxjs";
import {ITypedApiResponse} from "../../Interfaces/IApiResponse";
import {LoginResponseDto} from "../../Models/LoginResponseDto";

export interface IAuthenticationService {
  IsUsernameAvailable(username: string): Observable<ITypedApiResponse<boolean>>;

  //TODO: create a model or download an NPM module to get the data object for this response
  GetCredentialOptions(username: string): Observable<ITypedApiResponse<any>>;

  MakeCredential(makeCredentialRequestBody: any): Observable<ITypedApiResponse<LoginResponseDto>>;

  GetAssertionOptions(username: string): Observable<ITypedApiResponse<any>>;

  MakeAssertion(makeAssertionRequestBody: any): Observable<ITypedApiResponse<LoginResponseDto>>;
}
