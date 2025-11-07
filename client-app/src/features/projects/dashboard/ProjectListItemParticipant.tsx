import { observer } from "mobx-react-lite";
import { Image, List, Popup } from "semantic-ui-react";
import { Profile } from "../../../app/models/profile";
import { Link } from "react-router-dom";
import ProfileCard from "../../profiles/ProfileCard";

interface Props {
  participants: Profile[];
}

export default observer(function ProjectListItemParticipant({ participants }: Props) {
  return (
    <List horizontal>
      {participants.map((participant) => (
        <Popup
          hoverable
          key={participant.username}
          trigger={
            <List.Item key={participant.username} as={Link} to={`/profiles/${participant.username}`}>
              <Image size="mini" circular src="assets/user.png" />
            </List.Item>
          }
        >
          <Popup.Content>
            <ProfileCard profile={participant} />
          </Popup.Content>
        </Popup>
      ))}
    </List>
  );
});
