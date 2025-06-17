using AutoMapper;
using BLL.DTOs;
using DAL.Entities;
using DAL.Reposistories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class FeedbackService
    {
        private readonly IGenericRepository<Feedback> _feedbackRepository;
        private readonly IMapper mapper;
        public FeedbackService(IGenericRepository<Feedback> feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            return _feedbackRepository.GetAll();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(string feedbackId)
        {
            if (string.IsNullOrEmpty(feedbackId))
            {
                throw new ArgumentNullException(nameof(feedbackId));
            }

            var feedback = _feedbackRepository.GetSingle(f => f.FeedbackId == feedbackId);

            return feedback;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackByProductIdAsync(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                throw new ArgumentNullException(nameof(productId));
            }

            var feedbacks = _feedbackRepository.Get(f => f.ProductId == productId);

            return feedbacks;
        }

        public async Task<ResponseDTO> CreateFeedback(FeedbackRequestDTO feedbackRequest)
        {
            if (feedbackRequest == null)
            {
                throw new ArgumentNullException(nameof(feedbackRequest));
            }

            try
            {
                var feedback = mapper.Map<Feedback>(feedbackRequest);
                feedback.FeedbackId = Guid.NewGuid().ToString();
                feedback.CreatedAt = DateTime.UtcNow;
                _feedbackRepository.Create(feedback);
                return new ResponseDTO { Success = true, Message = "Feedback created successfully." };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Success = false, Message = $"An error occurred while creating the feedback: {ex.Message}" };
            }
        }

    }
}
