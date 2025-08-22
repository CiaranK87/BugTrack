import { observer } from "mobx-react-lite";
import { Button, Header, Item, Label, Segment } from "semantic-ui-react";
import { Project } from "../../../app/models/project";
import { Link } from "react-router-dom";
import { format } from "date-fns";
import { useStore } from "../../../app/stores/store";

interface Props {
  project: Project;
}

export default observer(function ProjectDetailedHeader({ project }: Props) {
  const {
    projectStore: { updateParticipants, loading, cancelProjectToggle },
  } = useStore();
  return (
    <Segment.Group>
      <Segment attached="top" style={{ padding: "0" }}>
        {project.isCancelled && (
          <Label style={{ position: "right", zIndex: 1000, left: -14, top: 10 }} ribbon color="red" content="Cancelled" />
        )}
        <Segment basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={project.projectTitle} style={{ color: "black" }} />
                <p>Start Date: {format(project.startDate!, "dd MMM yyyy")}</p>
                <p>
                  Project Owned by{" "}
                  <strong>
                    <Link to={`/profile/${project.owner?.username || ''}`}>
                      {project.owner?.displayName || project.projectOwner || 'Unknown'}
                    </Link>
                  </strong>
              </p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        {project.isOwner ? (
          <>
            <Button
              color={project.isCancelled ? "green" : "red"}
              floated="left"
              basic
              content={project.isCancelled ? "Reopen Project" : "Cancel Project"}
              onClick={cancelProjectToggle}
              loading={loading}
            />

            <Button disabled={project.isCancelled} as={Link} to={`/manageProject/${project.id}`} color="orange" floated="right">
              Manage Project
            </Button>
          </>
        ) : project.isParticipant ? (
          <Button loading={loading} onClick={updateParticipants}>
            Cancel participation
          </Button>
        ) : (
          <Button disabled={project.isCancelled} loading={loading} onClick={updateParticipants} color="teal">
            Join Project
          </Button>
        )}
      </Segment>
    </Segment.Group>
  );
});
