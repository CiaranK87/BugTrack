import { observer } from "mobx-react-lite";
import { Button, Header, Item, Label, Segment, Dropdown } from "semantic-ui-react";
import { Project } from "../../../app/models/project";
import { Link } from "react-router-dom";
import { format } from "date-fns";
import { useStore } from "../../../app/stores/store";

interface Props {
  project: Project;
}

export default observer(function ProjectDetailedHeader({ project }: Props) {
  const {
    projectStore: { loading, cancelProjectToggle, currentUserCanManage },
    userStore: { isAdmin }
  } = useStore();
  return (
    <Segment.Group className="project-detailed-header">
      <Segment attached="top" style={{ padding: "0" }}>
        {project.isCancelled && (
          <Label style={{ position: "absolute", zIndex: 1000, left: -14, top: 10 }} ribbon color="red" content="Cancelled" />
        )}
        <Segment basic>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
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

            <div className="mobile-only">
              {(project.isOwner || currentUserCanManage || isAdmin) && (
                <Dropdown
                  icon='setting'
                  className='icon'
                  button
                  basic
                  compact
                >
                  <Dropdown.Menu direction='left'>
                    <Dropdown.Header icon='options' content='Project Tools' />
                    <Dropdown.Divider />
                    <Dropdown.Item
                      as={Link}
                      to={`/manageProject/${project.id}`}
                      icon='edit'
                      text='Manage Project'
                      disabled={project.isCancelled}
                    />
                    {(project.isOwner || isAdmin) && (
                      <Dropdown.Item
                        icon={project.isCancelled ? 'play' : 'stop'}
                        text={project.isCancelled ? 'Reopen Project' : 'Cancel Project'}
                        onClick={cancelProjectToggle}
                        style={{ color: project.isCancelled ? 'green' : 'red' }}
                      />
                    )}
                  </Dropdown.Menu>
                </Dropdown>
              )}
            </div>
          </div>
        </Segment>
      </Segment>

      <Segment clearing attached="bottom" className="tablet-desktop-only">
        {(project.isOwner || currentUserCanManage || isAdmin) && (
          <>
            {(project.isOwner || isAdmin) && (
              <Button
                color={project.isCancelled ? "green" : "red"}
                floated="left"
                basic
                content={project.isCancelled ? "Reopen Project" : "Cancel Project"}
                onClick={cancelProjectToggle}
                loading={loading}
              />
            )}

            <Button disabled={project.isCancelled} as={Link} to={`/manageProject/${project.id}`} color="orange" floated="right">
              Manage Project
            </Button>
          </>
        )}
      </Segment>
    </Segment.Group>
  );
});
