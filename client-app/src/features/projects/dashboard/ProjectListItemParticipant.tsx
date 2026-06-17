import { observer } from "mobx-react-lite";
import { List, Popup } from "semantic-ui-react";
import { Profile } from "../../../app/models/profile";
import { Link } from "react-router-dom";
import ProfileCard from "../../profiles/ProfileCard";
import UserAvatar from "../../../app/common/UserAvatar";

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
              <UserAvatar image={participant.image} displayName={participant.displayName} size="mini" />
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
