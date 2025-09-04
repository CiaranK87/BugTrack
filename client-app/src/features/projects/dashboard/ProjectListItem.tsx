import { Link } from "react-router-dom";
import { Button, Icon, Item, Label, Segment } from "semantic-ui-react";
import { Project } from "../../../app/models/project";
import ProjectListItemParticipant from "./ProjectListItemParticipant";
import { format } from "date-fns";

interface Props {
  project: Project;
}

export default function ProjectListItem({ project }: Props) {
  return (
    <Segment.Group style={{ marginBottom: "30px" }}>
      <Segment>
        {project.isCancelled && <Label attached="top" color="red" content="Cancelled" style={{ textAlign: "center" }} />}
        <Item.Group>
          <Item>
            <Icon style={{ padding: 3 }} size="big" name="clipboard" circular />
            <Item.Content style={{ paddingLeft: "2em" }}>
              <Item.Header as={Link} to={`/projects/${project.id}`}>
                {project.projectTitle}
              </Item.Header>
              <Item.Description>Project owner - {project.owner!.displayName ?? "unknown"}</Item.Description>
              {project.isOwner && (
                <Item.Description>
                  <Label basic color="orange">
                    OWNER
                  </Label>
                </Item.Description>
              )}

              <Label basic color='orange' attached="top right">
                {`${project.ticketCount || 0} ticket${project.ticketCount === 1 ? '' : 's'}`}
              </Label>

              
              {project.isParticipant && !project.isOwner && (
                <Item.Description>
                  <Label basic color="green">
                    PARTICIPANT
                  </Label>
                </Item.Description>
              )}
            </Item.Content>
          </Item>
        </Item.Group>
      </Segment>
      <Segment>
        <Icon name="clock" /> {format(project.startDate!, "dd MMM yyyy")}
      </Segment>
      <Segment secondary>
        <ProjectListItemParticipant participants={project.participants!} />
      </Segment>
      <Segment clearing>
        <span>description - {project.description}</span>
        <Button as={Link} to={`/projects/${project.id}`} color="teal" floated="right" content="View" />
      </Segment>
    </Segment.Group>
  );
}
