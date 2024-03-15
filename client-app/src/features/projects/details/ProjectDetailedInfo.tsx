import { Segment, List, Label, Item, Image } from "semantic-ui-react";
import { Link } from "react-router-dom";
import { observer } from "mobx-react-lite";
import { Project } from "../../../app/models/project";

interface Props {
  project: Project;
}

export default observer(function ProjectDetailedInfo({ project: { participants, owner } }: Props) {
  if (!participants) return null;
  return (
    <>
      <Segment textAlign="center" style={{ border: "none" }} attached="top" secondary inverted color="teal">
        {participants.length} {participants.length === 1 ? "Participant" : "Participants"} participating
      </Segment>
      <Segment attached>
        <List relaxed divided>
          {participants.map((participant) => (
            <Item style={{ position: "relative" }} key={participant.username}>
              {participant.username === owner?.username && (
                <Label style={{ position: "absolute" }} color="orange" ribbon="right">
                  Owner
                </Label>
              )}
              <Image size="tiny" src={participant.image || "/assets/user.png"} />
              <Item.Content verticalAlign="middle">
                <Item.Header as="h3">
                  <Link to={`/profiles/${participant.username}`}>{participant.displayName}</Link>
                </Item.Header>
                <Item.Extra style={{ color: "orange" }}>project participant</Item.Extra>
              </Item.Content>
            </Item>
          ))}
        </List>
      </Segment>
    </>
  );
});
