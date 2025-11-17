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
        <Button.Group widths="10">
          <Button onClick={handleNotFound} content="Not Found" basic primary />
          <Button onClick={handleBadRequest} content="Bad Request" basic primary />
          <Button onClick={handleProjectValidationError} content="Project Validation Error" basic primary />
          <Button onClick={handleTicketValidationError} content="Ticket Validation Error" basic primary />
          <Button onClick={handleServerError} content="Server Error" basic primary />
          <Button onClick={handleUnauthorized} content="Unauthorized" basic primary />
          <Button onClick={handleBadProjectGuid} content="Bad Project Guid" basic primary />
          <Button onClick={handleBadTicketGuid} content="Bad Ticket Guid" basic primary />
        </Button.Group>
      </Segment>
      {errors && <ValidationError errors={errors} />}
    </>
  );
}
