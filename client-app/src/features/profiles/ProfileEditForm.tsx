import { observer } from "mobx-react-lite";
import { Form, Formik } from "formik";
import { Button, Header } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";
import MyTextInput from "../../app/common/form/MyTextInput";
import MyTextArea from "../../app/common/form/MyTextArea";
import * as Yup from 'yup';
import { Profile } from "../../app/models/profile";

interface Props {
    profile: Profile;
}

export default observer(function ProfileEditForm({ profile }: Props) {
    const { userStore, modalStore } = useStore();

    const validationSchema = Yup.object({
        displayName: Yup.string().required('Display name is required'),
    });

    
        return (
            <Formik
            initialValues={{
            displayName: profile.displayName || '',
            bio: profile.bio || ''
            }}
            onSubmit={(values) =>
            userStore.updateProfile(values).then(() => modalStore.closeModal())
            }
            validationSchema={validationSchema}
            >
            {({ handleSubmit, isSubmitting }) => (
            <Form className="ui form" onSubmit={handleSubmit} autoComplete="off">
            <Header content="Edit Profile" color="teal" />
            <MyTextInput placeholder="Display Name" name="displayName" />
            <MyTextArea rows={4} placeholder="Bio" name="bio" />
                        
            <div className="profile-edit-buttons">
              <Button
                type="button"
                content="Cancel"
                onClick={() => modalStore.closeModal()}
              />
              <Button
                positive
                type="submit"
                loading={isSubmitting}
                content="Update Profile"
              />
            </div>
            </Form>
          )}
        </Formik>
    );
});
