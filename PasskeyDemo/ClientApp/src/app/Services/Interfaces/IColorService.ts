import {Observable} from "rxjs";

export interface IColorService {
  GetColor(userId: string): Observable<string>;
  UpdateColor(userId: string, color: string): Observable<void>;
}
