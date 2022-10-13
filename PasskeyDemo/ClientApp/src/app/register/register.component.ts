import { Component, OnInit } from '@angular/core';
import {TestingAuthenticationService} from "../Services/authentication.service";
import {ConvertService} from "../Services/convert.service";
import {firstValueFrom} from "rxjs";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  username: string = "";

  constructor(
    private authService: TestingAuthenticationService,
    private convertService: ConvertService
  ) { }

  ngOnInit(): void {
  }

  async Register(): Promise<void>{
    let isUsernameAvailable = await firstValueFrom(this.authService.IsUsernameAvailable(this.username));

    if (!isUsernameAvailable) return;

    let credentialOptions = await firstValueFrom(this.authService.MakeCredentialOptions(this.username));

    console.log("Credential Options:")
    console.log(credentialOptions);

    let credential = await this.MakeCredential(credentialOptions);


    console.log("Credential:")
    console.log(credential);

    credentialOptions.challenge = this.convertService.CoerceToBase64Url(credentialOptions.challenge);
    credentialOptions.user.id = this.convertService.CoerceToBase64Url(credentialOptions.user.id);
    let rawId = this.convertService.CoerceToBase64Url(credential.rawId);
    let attestationObject = this.convertService.CoerceToBase64Url(credential.response.attestationObject);
    let clientDataJson = this.convertService.CoerceToBase64Url(credential.response.clientDataJSON);

    let requestBody = {
      id: credential.id,
      rawId: rawId,
      attestationObject: attestationObject,
      clientDataJSON: clientDataJson,
      options: credentialOptions
    }

    console.log("Request Body:")
    console.log(requestBody)

    let response = await firstValueFrom(this.authService.MakeCredential(requestBody));

    console.log(response);
  }

  MakeCredential(credentialOptions: any): Promise<any> {
    try {
      console.log("Making the Credential");

      // let credential = navigator.credentials.create({
      //   publicKey: {
      //     challenge: Uint8Array.from(credentialOptions.challenge),
      //     rp: {
      //       name: "Rick Bordelon",
      //       // id: "Localhost",
      //     },
      //     user: {
      //       id: Uint8Array.from(credentialOptions.user.id),
      //       name: credentialOptions.user.name,
      //       displayName: credentialOptions.user.displayName
      //     },
      //     pubKeyCredParams: [{alg: -7, type: "public-key"}],
      //     timeout: 60000
      //   }
      // });

      credentialOptions.challenge = Uint8Array.from(credentialOptions.challenge);
      credentialOptions.user.id = Uint8Array.from(credentialOptions.user.id);

      let credential = navigator.credentials.create({
        publicKey: credentialOptions
      });



      return credential;
    }
    catch (e) {
      console.log(e);
      return new Promise<any>(() => {});
    }

  }

}
