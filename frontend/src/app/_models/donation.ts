import { UserDetail } from "./userDetail";

export interface Donation {
    donationId: number;
    noOfMeals: number;
    status: string;
    feedbackByDonor: string;
    feedbackByCollector: string;
    Donor: UserDetail;
    Collector?: UserDetail;
}