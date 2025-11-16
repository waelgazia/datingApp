import { ResourceBase } from "../base/ResourceBase";
import { LIKE_PREDICATES } from "../../constants/like-predicates";

export class LikesParameters extends ResourceBase {
  predicate: string = LIKE_PREDICATES.LIKED;
}