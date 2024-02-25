import { observer } from "mobx-react-lite";
import { Button, Header, Item, Segment } from "semantic-ui-react";
import { Project } from "../../../app/models/project";
import { Link } from "react-router-dom";
import { format } from "date-fns";

interface Props {
  project: Project;
}

export default observer(function ProjectDetailedHeader({ project }: Props) {
  return (
    <Segment.Group>
      <Segment attached="top" style={{ padding: "0" }}>
        <Segment basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={project.projectTitle} style={{ color: "black" }} />
                <p>Start Date: {format(project.startDate!, "dd MMM yyyy")}</p>
                <p>
                  Project Owned by <strong>{project.projectOwner}</strong>
                </p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        <Button as={Link} to={`/manage/${project.id}`} color="orange" floated="right">
          Manage Project
        </Button>
      </Segment>
    </Segment.Group>
  );
});
