export interface UserDto {
  id: string,
  displayName: string,
  email: string,
  token: string,
  imageUrl? : string,
  roles: string[]
}