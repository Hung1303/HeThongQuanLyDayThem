using BusinessObjects;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void GenerateRefreshToken(Token token)
        {
            if (token.Id == Guid.Empty)
            {
                token.Id = Guid.NewGuid();
            }
            try
            {
                if (token.ExpiredTime.HasValue && (token.ExpiredTime.Value.Kind == DateTimeKind.Local || token.ExpiredTime.Value.Kind == DateTimeKind.Unspecified))
                {
                    token.ExpiredTime = token.ExpiredTime.Value.ToUniversalTime();
                }
                var existingToken = _unitOfWork.GetRepository<Token>().Get(x => x.UserId == token.UserId);
                if (existingToken != null)
                {
                    existingToken.AccessToken = token.AccessToken;
                    existingToken.RefreshToken = token.RefreshToken;
                    existingToken.ExpiredTime = token.ExpiredTime;
                    existingToken.Status = token.Status;
                    _unitOfWork.GetRepository<Token>().Update(existingToken);
                }
                else
                {
                    _unitOfWork.GetRepository<Token>().Insert(token);
                }
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Token GetRefreshToken(string refreshToken)
        {
            try
            {
                var _refreshToken = _unitOfWork.GetRepository<Token>().Get(x => x.RefreshToken == refreshToken);
                return _refreshToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Token GetRefreshTokenByUserID(Guid id)
        {
            try
            {
                var existingToken = _unitOfWork.GetRepository<Token>().Get(x => x.UserId == id);
                return existingToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveAllRefreshToken()
        {
            try
            {
                var refreshToken = _unitOfWork.GetRepository<Token>().Get();
                foreach (var item in refreshToken)
                {
                    _unitOfWork.GetRepository<Token>().Delete(item);
                }
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void ResetRefreshToken()
        {
            try
            {
                var _refreshToken = _unitOfWork.GetRepository<Token>().Get();
                foreach (var item in _refreshToken)
                {
                    if (item.Status == 2 || item.ExpiredTime <= DateTime.Now)
                    {
                        _unitOfWork.GetRepository<Token>().Delete(item);
                        _unitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateRefreshToken(Token refreshToken)
        {
            try
            {
                refreshToken.Status = 2;
                _unitOfWork.GetRepository<Token>().Update(refreshToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
