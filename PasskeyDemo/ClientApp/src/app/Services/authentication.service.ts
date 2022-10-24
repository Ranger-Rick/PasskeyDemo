import {Inject, Injectable} from '@angular/core';
import {IAuthenticationService} from "./Interfaces/IAuthenticationService";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {LoginResponseDto} from "../Models/LoginResponseDto";
import {ITypedApiResponse} from "../Interfaces/IApiResponse";

@Injectable({
  providedIn: 'root'
})
export class TestingAuthenticationService implements IAuthenticationService {

  baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl + "Authentication/";
  }

  IsUsernameAvailable(username: string): Observable<ITypedApiResponse<boolean>> {

    return this.http.get<ITypedApiResponse<boolean>>(this.baseUrl + "UsernameAvailable?username=" + username);
  }

  GetCredentialOptions(username: string): Observable<ITypedApiResponse<any>> {
    return this.http.get<ITypedApiResponse<any>>(this.baseUrl + "GetCredentialOptions?username=" + username);
  }

  MakeCredential(makeCredentialRequestBody: any): Observable<ITypedApiResponse<LoginResponseDto>> {
      return this.http.post<ITypedApiResponse<LoginResponseDto>>(this.baseUrl + "MakeCredential", makeCredentialRequestBody);
  }

  GetAttestationOptions(username: string): Observable<ITypedApiResponse<any>>{
    return this.http.get<ITypedApiResponse<any>>(this.baseUrl + "GetAssertionOptions?username=" + username);
  }

  MakeAssertion(makeAssertionRequestBody: any): Observable<ITypedApiResponse<LoginResponseDto>> {
    return this.http.post<ITypedApiResponse<LoginResponseDto>>(this.baseUrl + "MakeAssertion", makeAssertionRequestBody);
  }
}
