export interface Incident {
    id?: number;
    type: string;
    location: string;
    description: string;
    submittedAt?: Date;
}