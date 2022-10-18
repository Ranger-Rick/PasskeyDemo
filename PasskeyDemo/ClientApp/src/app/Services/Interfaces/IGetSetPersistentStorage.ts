import {LoginResponseDto} from "../../Models/LoginResponseDto";

export interface IGetSetPersistentStorage {
  SetPersistentProperties(options: LoginResponseDto): void;
  ClearPersistentProperties(): void;
}
