namespace CMS.DATA.Entities
{
    public class UserQuizTaken : BaseEntity
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Preferredanswer { get; set; }

        public string CorrectAnswer { get; set; }

        public string QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
        public bool CompletionStatus { get; set; }
    }
}