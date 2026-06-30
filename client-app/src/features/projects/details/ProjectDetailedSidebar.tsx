import { Segment, List, Item } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { Profile } from "../../../app/models/profile";
import UserAvatar from "../../../app/common/UserAvatar";

interface Props {
  participants: Profile[];
}

export default observer(function ProjectDetailedSidebar({ participants }: Props) {
  return (
    <>
      <Segment textAlign="center" style={{ border: "none" }} attached="top" secondary inverted color="teal">
        {participants.length} {participants.length === 1 ? "Participant" : "Participants"} participating
      </Segment>
      <Segment attached>
        <List relaxed divided>
          {participants.map((participant) => (
            <Item style={{ position: "relative" }} key={participant.username}>
              <UserAvatar image={participant.image} displayName={participant.displayName} size="tiny" />
              <Item.Content verticalAlign="middle">
                <Item.Header as="h3">
                  <Link to={`/profile/${participant.username}`}>{participant.displayName}</Link>
                </Item.Header>
                <Item.Extra style={{ color: "orange" }}>Following</Item.Extra>
              </Item.Content>
            </Item>
          ))}
        </List>
      </Segment>
    </>
  );
});
