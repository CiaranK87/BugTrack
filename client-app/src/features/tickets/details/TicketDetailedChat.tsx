import { observer } from "mobx-react-lite";
import { Segment, Header, Comment, Form, Button } from "semantic-ui-react";
import UserAvatar from "../../../app/common/UserAvatar";

export default observer(function TicketDetailedChat() {
  return (
    <>
      <Segment textAlign="center" attached="top" inverted color="teal" style={{ border: "none" }}>
        <Header>Chat about this ticket</Header>
      </Segment>
      <Segment attached>
        <Comment.Group>
          <Comment>
            <Comment.Avatar as="div"><UserAvatar image={undefined} displayName="Matt" size="mini" /></Comment.Avatar>
            <Comment.Content>
              <Comment.Author as="a">Matt</Comment.Author>
              <Comment.Metadata>
                <div>Today at 5:42PM</div>
              </Comment.Metadata>
              <Comment.Text>Weird bug!</Comment.Text>
              <Comment.Actions>
                <Comment.Action>Reply</Comment.Action>
              </Comment.Actions>
            </Comment.Content>
          </Comment>

          <Comment>
            <Comment.Avatar as="div"><UserAvatar image={undefined} displayName="Joe Henderson" size="mini" /></Comment.Avatar>
            <Comment.Content>
              <Comment.Author as="a">Joe Henderson</Comment.Author>
              <Comment.Metadata>
                <div>5 days ago</div>
              </Comment.Metadata>
              <Comment.Text>Dude, this is wrecking my head</Comment.Text>
              <Comment.Actions>
                <Comment.Action>Reply</Comment.Action>
              </Comment.Actions>
            </Comment.Content>
          </Comment>

          <Form reply>
            <Form.TextArea />
            <Button content="Add Reply" labelPosition="left" icon="edit" primary />
          </Form>
        </Comment.Group>
      </Segment>
    </>
  );
});
