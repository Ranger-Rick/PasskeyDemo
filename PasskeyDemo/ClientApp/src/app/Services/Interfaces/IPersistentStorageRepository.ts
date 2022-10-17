export interface IPersistentStorageRepository {
  SetValue(key: string, value: string): void;

  GetValue<Type>(key: string): Type;
}
