import { ResourceBase } from "../base/ResourceBase";
import { MESSAGES_CONTAINERS } from "../../constants/messages-containers";

export class MessagesParameters extends ResourceBase {
  container: string = MESSAGES_CONTAINERS.INBOX;
}