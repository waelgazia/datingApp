import { ResourceBase } from "../base/ResourceBase";

export class MembersParameters extends ResourceBase {
  gender?: string;
  minAge = 18;
  maxAge = 100;
  orderBy = 'lastActive';
}