import {Inject, Injectable} from '@angular/core';
import {IAuthenticationService} from "./Interfaces/IAuthenticationService";
import {Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class TestingAuthenticationService implements IAuthenticationService {

  baseUrl: string = "";

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl + "Authentication/";
  }

  IsUsernameAvailable(username: string): Observable<boolean> {

    return this.http.get<boolean>(this.baseUrl + "UsernameAvailable?username=" + username);
  }

  MakeCredentialOptions(username: string): Observable<any> {
    return this.http.get<boolean>(this.baseUrl + "GetCredentialOptions?username=" + username);
  }

  MakeCredential(makeCredentialRequestBody: any): Observable<any> {
      return this.http.post<any>(this.baseUrl + "MakeCredential", makeCredentialRequestBody);
  }

  GetAttestationOptions(username: string): Observable<any>{
    return this.http.get(this.baseUrl + "GetAssertionOptions?username=" + username);
  }

  MakeAssertion(makeAssertionRequestBody: any): Observable<any> {
    return this.http.post<any>(this.baseUrl + "MakeAssertion", makeAssertionRequestBody);
  }
}
