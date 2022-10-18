import { Injectable } from '@angular/core';
import jwt_decode from "jwt-decode"
import {IJsonWebTokenService} from "./Interfaces/IJsonWebTokenService";
import {BrowserStorageService} from "./browser-storage.service";
import {Constants} from "../Constants";

@Injectable({
  providedIn: 'root'
})
export class JsonWebTokenService implements IJsonWebTokenService{

  constructor(private persistentStorage: BrowserStorageService) { }

  IsLoggedIn(): boolean {
    try {
      let token = this.persistentStorage.GetValue<string>(Constants.Token);

      let decodedToken = jwt_decode(token) as IJsonWebTokenDecode;

      let now = new Date(Date.now());
      let expiration = new Date(decodedToken.exp * 1000);

      return now < expiration;
    }
    catch{
      return false;
    }
  }
}

interface IJsonWebTokenDecode {
  exp: number
}
