import { Header, Message, Button, Icon, Form } from "semantic-ui-react";
import { useTheme } from "../../app/context/ThemeContext";
import { useState } from "react";
import { toast } from "react-toastify";
import axios from "axios";

export default function ContactMessage() {
  const { darkMode } = useTheme();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    message: ""
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.name || !formData.email || !formData.message) {
      toast.error("Please fill in all fields");
      return;
    }

    setIsSubmitting(true);
    
    try {
      // This will send the request to your backend API using the configured axios instance
      const response = await axios.post('/contact/request-access', {
        name: formData.name,
        email: formData.email,
        message: formData.message
      });

      if (response.status === 200) {
        setShowSuccess(true);
        setFormData({ name: "", email: "", message: "" });
        // Hide success message after 5 seconds
        setTimeout(() => setShowSuccess(false), 5000);
      } else {
        toast.error("Failed to send request. Please try again later.");
      }
    } catch (error) {
      toast.error("Failed to send request. Please try again later.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={{
      textAlign: 'center',
      padding: '20px',
      color: darkMode ? '#e9ecef' : '#212529'
    }}>
      {showSuccess && (
        <Message
          success
          style={{
            marginBottom: '20px',
            backgroundColor: darkMode ? '#2d5a3d' : undefined,
            color: darkMode ? '#ffffff' : undefined,
            borderColor: darkMode ? '#4a7c59' : undefined
          }}
        >
          <Message.Header style={{ color: darkMode ? '#ffffff' : undefined }}>
            Request Sent Successfully!
          </Message.Header>
          <p style={{ color: darkMode ? '#e9ecef' : undefined }}>
            Your access request has been received. We'll review it and get back to you soon.
          </p>
        </Message>
      )}
      <Header 
        as="h2" 
        content="Request Access to BugTrack" 
        color={darkMode ? undefined : "teal"} 
        textAlign="center" 
        style={{ color: darkMode ? '#ffffff' : undefined }}
      />
      <Message 
        info={!darkMode}
        style={{
          backgroundColor: darkMode ? 'var(--dm-bg-secondary)' : undefined,
          color: darkMode ? 'var(--dm-text-secondary)' : undefined,
          borderColor: darkMode ? 'var(--dm-border)' : undefined
        }}
      >
        <Message.Header style={{ color: darkMode ? '#ffffff' : undefined }}>
          Access to this application is by invitation only
        </Message.Header>
        <p style={{ color: darkMode ? 'var(--dm-text-secondary)' : undefined }}>
          Please fill out the form below to request access to BugTrack.
          Include a brief description of why you'd like access and any relevant background information.
        </p>
      </Message>
      
      <Form 
        onSubmit={handleSubmit}
        style={{ 
          textAlign: 'left', 
          maxWidth: '500px', 
          margin: '0 auto',
          backgroundColor: darkMode ? 'var(--dm-bg-secondary)' : undefined,
          padding: '20px',
          borderRadius: '8px',
          border: darkMode ? '1px solid var(--dm-border)' : undefined
        }}
      >
        <Form.Input
          fluid
          label="Name"
          placeholder="Your full name"
          name="name"
          value={formData.name}
          onChange={handleChange}
          required
          style={{ color: darkMode ? 'var(--dm-text-secondary)' : undefined }}
        />
        <Form.Input
          fluid
          label="Email"
          placeholder="Your email address"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          required
          style={{ color: darkMode ? 'var(--dm-text-secondary)' : undefined }}
        />
        <Form.TextArea
          label="Message"
          placeholder="Tell us why you'd like access to BugTrack..."
          name="message"
          value={formData.message}
          onChange={handleChange}
          required
          rows={4}
          style={{ 
            color: darkMode ? 'var(--dm-text-secondary)' : undefined,
            backgroundColor: darkMode ? 'var(--dm-bg-tertiary)' : undefined
          }}
        />
        <Button 
          primary 
          type="submit"
          loading={isSubmitting}
          disabled={isSubmitting}
          icon
          labelPosition="left"
          size="large"
          fluid
        >
          <Icon name="mail" />
          Send Request
        </Button>
      </Form>
    </div>
  );
}