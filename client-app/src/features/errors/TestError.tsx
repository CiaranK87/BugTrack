import { Button, Header, Segment } from "semantic-ui-react";
import axios from "axios";
import { useState } from "react";
import ValidationError from "./ValidationError";
import { logger } from "../../app/utils/logger";

export default function TestErrors() {
  const baseUrl = "http://localhost:5000/api/";
  const [errors, setErrors] = useState(null);

  function handleNotFound() {
    axios.get(baseUrl + "buggy/not-found").catch((err) => logger.debug("Not found error", err.response));
  }

  function handleBadRequest() {
    axios.get(baseUrl + "buggy/bad-request").catch((err) => logger.debug("Bad request error", err.response));
  }

  function handleServerError() {
    axios.get(baseUrl + "buggy/server-error").catch((err) => logger.debug("Server error", err.response));
  }

  function handleUnauthorized() {
    axios.get(baseUrl + "buggy/unauthorized").catch((err) => logger.debug("Unauthorized error", err.response));
  }

  function handleBadProjectGuid() {
    axios.get(baseUrl + "projects/notaguid").catch((err) => logger.debug("Bad project GUID error", err.response));
  }
  function handleBadTicketGuid() {
    axios.get(baseUrl + "tickets/notaguid").catch((err) => logger.debug("Bad ticket GUID error", err.response));
  }

  function handleProjectValidationError() {
    axios.post(baseUrl + "projects", {}).catch((err) => setErrors(err));
  }

  function handleTicketValidationError() {
    axios.post(baseUrl + "tickets", {}).catch((err) => setErrors(err));
  }

  return (
    <>
      <Header as="h1" content="Test Error component" />
      <Segment>
        <Button.Group widths="10" className="test-error-group">
          <Button onClick={handleNotFound} content="Not Found" color="blue" />
          <Button onClick={handleBadRequest} content="Bad Request" color="orange" />
          <Button onClick={handleProjectValidationError} content="Project Validation" color="teal" />
          <Button onClick={handleTicketValidationError} content="Ticket Validation" color="purple" />
          <Button onClick={handleServerError} content="Server Error" color="red" />
          <Button onClick={handleUnauthorized} content="Unauthorized" color="yellow" />
          <Button onClick={handleBadProjectGuid} content="Bad Project Guid" color="brown" />
          <Button onClick={handleBadTicketGuid} content="Bad Ticket Guid" color="grey" />
        </Button.Group>
      </Segment>
      {errors && <ValidationError errors={errors} />}
    </>
  );
}
