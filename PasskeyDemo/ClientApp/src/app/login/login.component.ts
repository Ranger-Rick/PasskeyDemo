import { Component, OnInit } from '@angular/core';
import {TestingAuthenticationService} from "../Services/authentication.service";
import {ConvertService} from "../Services/convert.service";
import {firstValueFrom} from "rxjs";
import {Router} from "@angular/router";
import {PersistentPropertiesService} from "../Services/persistent-properties.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  username: string = "";

  constructor(
    private route: Router,
    private authService: TestingAuthenticationService,
    private convertService: ConvertService,
    private persistentStorage: PersistentPropertiesService
  ) { }

  ngOnInit(): void {
    this.persistentStorage.ClearPersistentProperties()
  }

  async Login(): Promise<void>{
    let assertionOptions = await firstValueFrom(this.authService.GetAttestationOptions(this.username));

    let newAssertionOptions = assertionOptions;

    newAssertionOptions.challenge = this.convertService.CoerceToBase64(assertionOptions.challenge);

    //This code is in the official Fido2 demo project. I have no idea what it is doing but it smells really bad
    // fix escaping. Change this to coerce
    newAssertionOptions.allowCredentials.forEach(function (listItem: any) {
      var fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
      listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
    });

    let credential = await navigator.credentials.get({ publicKey: newAssertionOptions });

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

    let requestBody = {
      id: id,
      authenticatorData: authData,
      signature: signature,
      userHandle: userHandle,
      rawId: rawId,
      clientDataJson: clientDataJson,
      assertionOptions: options
    }

    let response = await firstValueFrom(this.authService.MakeAssertion(requestBody));

    if (!response.executedSuccessfully) {
      alert("Unable to verify your Passkey");
      return;
    }

    this.persistentStorage.SetPersistentProperties(response);

    await this.route.navigateByUrl("/circle");
  }

}
