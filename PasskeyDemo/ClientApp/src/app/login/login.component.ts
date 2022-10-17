import { Component, OnInit } from '@angular/core';
import {TestingAuthenticationService} from "../Services/authentication.service";
import {ConvertService} from "../Services/convert.service";
import {firstValueFrom} from "rxjs";
import {MakeAssertionResponseDto} from "../Models/MakeAssertionResponseDto";
import {BrowserStorageService} from "../Services/browser-storage.service";
import {Constants} from "../Constants";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  username: string = "";

  constructor(
    private authService: TestingAuthenticationService,
    private convertService: ConvertService,
    private persistentStorage: BrowserStorageService
  ) { }

  ngOnInit(): void {
    this.ClearPersistentProperties()
  }

  async Login(): Promise<void>{
    let assertionOptions = await firstValueFrom(this.authService.GetAttestationOptions(this.username));
    console.log("Assertion Options:");
    console.log(assertionOptions);

    let newAssertionOptions = assertionOptions;

    // todo: switch this to coercebase64
    const challenge = assertionOptions.challenge.replace(/-/g, "+").replace(/_/g, "/");
    newAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

    // fix escaping. Change this to coerce
    newAssertionOptions.allowCredentials.forEach(function (listItem: any) {
      var fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
      listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
    });

    let credential = await navigator.credentials.get({ publicKey: newAssertionOptions });

    console.log("Credential:")
    console.log(credential);

    await this.VerifyAssertionWithServer(credential, assertionOptions);
  }

  async VerifyAssertionWithServer(assertedCredential: any, options: any){

    options.challenge = this.convertService.CoerceToBase64Url(options.challenge);
    options.allowCredentials[0].id = this.convertService.CoerceToBase64Url(options.allowCredentials[0].id);


    let credentialResponse = assertedCredential.response;

    //Garbage
    let id = assertedCredential.id;
    let authData = this.convertService.CoerceToBase64Url(credentialResponse.authenticatorData);
    let clientDataJson = this.convertService.CoerceToBase64Url(credentialResponse.clientDataJSON);
    let rawId = this.convertService.CoerceToBase64Url(assertedCredential.rawId);
    let signature = this.convertService.CoerceToBase64Url(credentialResponse.signature);
    let userHandle = this.convertService.CoerceToBase64Url(credentialResponse.userHandle);

    var requestBody = {
      id: id,
      authenticatorData: authData,
      signature: signature,
      userHandle: userHandle,
      rawId: rawId,
      clientDataJson: clientDataJson,
      assertionOptions: options
    }

    console.log("Request Body:");
    console.log(requestBody);

    let response = await firstValueFrom(this.authService.MakeAssertion(requestBody));
    console.log("Assertion Response:");
    console.log(response);

    if (!response.executedSuccessfully) {
      alert("Unable to verify your Passkey");
      return;
    }

    this.SetPersistentProperties(response);
    
  }

  SetPersistentProperties(options: MakeAssertionResponseDto): void {
    this.persistentStorage.SetValue(Constants.UserId, options.userId);
    this.persistentStorage.SetValue(Constants.Username, options.username);
    this.persistentStorage.SetValue(Constants.DisplayName, options.displayName);
    this.persistentStorage.SetValue(Constants.Token, options.token);
  }

  ClearPersistentProperties(): void {
    this.persistentStorage.SetValue(Constants.UserId, "");
    this.persistentStorage.SetValue(Constants.Username, "");
    this.persistentStorage.SetValue(Constants.DisplayName, "");
    this.persistentStorage.SetValue(Constants.Token, "");
  }

}
