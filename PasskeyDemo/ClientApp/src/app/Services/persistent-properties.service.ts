import { Injectable } from '@angular/core';
import {IGetSetPersistentStorage} from "./Interfaces/IGetSetPersistentStorage";
import {LoginResponseDto} from "../Models/LoginResponseDto";
import {Constants} from "../Constants";
import {BrowserStorageService} from "./browser-storage.service";

@Injectable({
  providedIn: 'root'
})
export class PersistentPropertiesService implements IGetSetPersistentStorage{

  constructor(private persistentStorage: BrowserStorageService) { }

  ClearPersistentProperties(): void {
    this.persistentStorage.SetValue(Constants.UserId, "");
    this.persistentStorage.SetValue(Constants.Username, "");
    this.persistentStorage.SetValue(Constants.DisplayName, "");
    this.persistentStorage.SetValue(Constants.Token, "");
  }

  SetPersistentProperties(options: LoginResponseDto): void {
    this.persistentStorage.SetValue(Constants.UserId, options.userId);
    this.persistentStorage.SetValue(Constants.Username, options.username);
    this.persistentStorage.SetValue(Constants.DisplayName, options.displayName);
    this.persistentStorage.SetValue(Constants.Token, options.token);
  }
}
