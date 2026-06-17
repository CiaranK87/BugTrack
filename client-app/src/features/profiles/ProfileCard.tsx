import { observer } from "mobx-react-lite";
import { Link } from "react-router-dom";
import { Card, Icon } from "semantic-ui-react";
import { Profile } from "../../app/models/profile";
import UserAvatar from "../../app/common/UserAvatar";

interface Props {
  profile: Profile;
}

export default observer(function ProfileCard({ profile }: Props) {
  return (
    <Card as={Link} to={`/profile/${profile.username}`}>
      <UserAvatar image={profile.image} displayName={profile.displayName} size="small" circular={false} style={{ borderRadius: '4px 4px 0 0', width: '100%', height: '80px' }} />
      <Card.Content>
        <Card.Header>{profile.displayName}</Card.Header>
        <Card.Description>Bio goes here</Card.Description>
      </Card.Content>
      <Card.Content extra>
        <Icon name="user" />
        collaborating on X projects
      </Card.Content>
    </Card>
  );
});
