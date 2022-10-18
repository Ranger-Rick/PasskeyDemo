import {Component, OnInit} from '@angular/core';
import {TestingAuthenticationService} from "../Services/authentication.service";
import {ConvertService} from "../Services/convert.service";
import {firstValueFrom} from "rxjs";
import {PersistentPropertiesService} from "../Services/persistent-properties.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  username: string = "";

  constructor(
    private route: Router,
    private authService: TestingAuthenticationService,
    private convertService: ConvertService,
    private persistentStorage: PersistentPropertiesService
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

    if (!response.executedSuccessfully){
      alert("Unable to register your account");
      return;
    }

    this.persistentStorage.SetPersistentProperties(response);
    await this.route.navigateByUrl("/circle");
  }

  MakeCredential(credentialOptions: any): Promise<any> {
    try {
      console.log("Making the Credential");

      let enc = new TextEncoder();
      let userIdAsArrayBuffer = enc.encode(credentialOptions.user.id);

      credentialOptions.challenge = Uint8Array.from(credentialOptions.challenge);
      credentialOptions.user.id = userIdAsArrayBuffer;

      return navigator.credentials.create({
        publicKey: credentialOptions
      });
    }
    catch (e) {
      console.log(e);
      return new Promise<any>(() => {});
    }

  }

}
