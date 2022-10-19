import {Inject, Injectable} from '@angular/core';
import {IColorService} from "./Interfaces/IColorService";
import {Observable} from "rxjs";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {BrowserStorageService} from "./browser-storage.service";
import {Constants} from "../Constants";

@Injectable({
  providedIn: 'root'
})
export class ColorService implements IColorService{

  baseUrl: string;

  constructor(
    @Inject('BASE_URL') baseUrl: string,
    private http: HttpClient,
    private storage: BrowserStorageService)
  {
    this.baseUrl = baseUrl + "Color/"
  }

  GetColor(userId: string): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", "Bearer " + this.storage.GetValue<string>(Constants.Token));

    const requestOptions: Object = {
      headers: headers,
      responseType: 'text'
    }

    return this.http.get<string>(this.baseUrl + "GetColor?userId=" + userId, requestOptions);
  }

  UpdateColor(userId: string, color: string): Observable<void> {
    let headers = new HttpHeaders();
    headers = headers.set("Authorization", "Bearer " + this.storage.GetValue<string>(Constants.Token))

    return this.http.post<void>(
      this.baseUrl + `UpdateColor?userId=${userId}&color=${color}`,
      undefined,
      {
        "headers": headers
      });
  }
}
