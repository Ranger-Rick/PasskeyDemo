import {Observable} from "rxjs";

export interface IAuthenticationService {
  IsUsernameAvailable(username: string): Observable<boolean>;

  //TODO: create a model or download an NPM module to get the data object for this response
  MakeCredentialOptions(username: string): Observable<any>;

  MakeCredential(makeCredentialRequestBody: any): Observable<any>;

  GetAttestationOptions(username: string): Observable<any>;
}
